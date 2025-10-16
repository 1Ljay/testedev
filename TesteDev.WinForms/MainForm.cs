using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace TesteDev.WinForms
{
    public partial class MainForm : Form
    {
        private int? _selectedId = null;

        public MainForm() => InitializeComponent();

        private void MainForm_Load(object? sender, EventArgs e)
        {
            CarregarGrid(null);
            LimparCampos();
        }

        //- CARREGAR GRID (com filtro opcional) 
        private void CarregarGrid(string? termo)
        {
            using var con = new NpgsqlConnection(Program.ConnectionString);
            con.Open();

            string sql = @"
                SELECT id, nome, idade 
                FROM public.cadastro
                /**where**/
                ORDER BY id DESC";

            //- Monta filtro (nome LIKE, idade exata se for número)
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            if (!string.IsNullOrWhiteSpace(termo))
            {
                if (int.TryParse(termo.Trim(), out var idade))
                {
                    sql = sql.Replace("/**where**/", "WHERE idade = @idade");
                    cmd.Parameters.AddWithValue("@idade", idade);
                }
                else
                {
                    sql = sql.Replace("/**where**/", "WHERE unaccent(lower(nome)) LIKE unaccent(lower(@nome))");
                    cmd.Parameters.AddWithValue("@nome", $"%{termo.Trim()}%");
                }
            }
            else
            {
                sql = sql.Replace("/**where**/", "");
            }

            cmd.CommandText = sql;

            //- Se não tiver extensão unaccent, troca por lower(nome) LIKE lower(@nome)
            //- (Banco vai aceitar mesmo sem unaccent)
            cmd.CommandText = cmd.CommandText.Replace("unaccent(", "(");

            using var da = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            dgv.DataSource = dt;
            if (dgv.Columns.Contains("id")) dgv.Columns["id"].HeaderText = "ID";
            if (dgv.Columns.Contains("nome")) dgv.Columns["nome"].HeaderText = "Nome";
            if (dgv.Columns.Contains("idade")) dgv.Columns["idade"].HeaderText = "Idade";
        }

        private void LimparCampos()
        {
            _selectedId = null;
            txtNome.Text = "";
            numIdade.Value = 18;
            dgv.ClearSelection();
        }

        // - CLIQUES
        private void dgv_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].DataBoundItem is not DataRowView row) return;
            _selectedId = Convert.ToInt32(row["id"]);
            txtNome.Text = row["nome"]?.ToString() ?? "";
            numIdade.Value = Convert.ToDecimal(row["idade"]);
        }

        private void btnNovo_Click(object? sender, EventArgs e) => LimparCampos();

        private void btnSalvar_Click(object? sender, EventArgs e)
        {
            if (_selectedId is null) Inserir();
            else Atualizar();
        }

        private void btnExcluir_Click(object? sender, EventArgs e)
        {
            if (_selectedId is null)
            {
                MessageBox.Show("Selecione um registro na tabela.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Confirma exclusão?", "Excluir",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            using var con = new NpgsqlConnection(Program.ConnectionString);
            con.Open();
            using var cmd = new NpgsqlCommand("DELETE FROM public.cadastro WHERE id=@id;", con);
            cmd.Parameters.AddWithValue("@id", _selectedId.Value);

            try
            {
                var rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    CarregarGrid(txtBuscar.Text);
                    LimparCampos();
                }
            }
            catch (PostgresException ex) { TratarErroPg(ex); }
        }

        private void btnBuscar_Click(object? sender, EventArgs e) => CarregarGrid(txtBuscar.Text);
        private void txtBuscar_KeyDown(object? sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) CarregarGrid(txtBuscar.Text); }

        //- INSERT / UPDATE
        private void Inserir()
        {
            using var con = new NpgsqlConnection(Program.ConnectionString);
            con.Open();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO public.cadastro (nome, idade) VALUES (@nome, @idade) RETURNING id;", con);
            cmd.Parameters.AddWithValue("@nome", txtNome.Text.Trim());
            cmd.Parameters.AddWithValue("@idade", (int)numIdade.Value);

            try
            {
                var newId = (int)cmd.ExecuteScalar();
                MessageBox.Show($"Inserido com ID {newId}.", "OK",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CarregarGrid(txtBuscar.Text);
                LimparCampos();
            }
            catch (PostgresException ex) { TratarErroPg(ex); }
        }

        private void Atualizar()
        {
            if (_selectedId is null) return;

            using var con = new NpgsqlConnection(Program.ConnectionString);
            con.Open();
            using var cmd = new NpgsqlCommand(
                "UPDATE public.cadastro SET nome=@nome, idade=@idade WHERE id=@id;", con);
            cmd.Parameters.AddWithValue("@id", _selectedId.Value);
            cmd.Parameters.AddWithValue("@nome", txtNome.Text.Trim());
            cmd.Parameters.AddWithValue("@idade", (int)numIdade.Value);

            try
            {
                var rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    CarregarGrid(txtBuscar.Text);
                    LimparCampos();
                }
            }
            catch (PostgresException ex) { TratarErroPg(ex); }
        }

        //- ERROS DO PG 
        private void TratarErroPg(PostgresException ex)
        {
            string msg = ex.SqlState switch
            {
                "23502" => "Todos os campos são obrigatórios (NOT NULL).",
                "23505" => "A idade não pode se repetir (UNIQUE).",
                "23514" => "A idade deve ser maior que zero (CHECK).",
                _ => $"Erro do banco: {ex.MessageText}"
            };
            MessageBox.Show(msg, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void lblBuscar_Click(object sender, EventArgs e)
        {

        }
    }
}
