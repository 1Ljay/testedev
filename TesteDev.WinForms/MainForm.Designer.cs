namespace TesteDev.WinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.NumericUpDown numIdade;
        private System.Windows.Forms.Button btnNovo;
        private System.Windows.Forms.Button btnExcluir;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.Label lblIdade;
        private System.Windows.Forms.Label lblBuscar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtNome = new System.Windows.Forms.TextBox();
            numIdade = new System.Windows.Forms.NumericUpDown();
            btnNovo = new System.Windows.Forms.Button();
            btnExcluir = new System.Windows.Forms.Button();
            btnSalvar = new System.Windows.Forms.Button();
            txtBuscar = new System.Windows.Forms.TextBox();
            btnBuscar = new System.Windows.Forms.Button();
            dgv = new System.Windows.Forms.DataGridView();
            lblNome = new System.Windows.Forms.Label();
            lblIdade = new System.Windows.Forms.Label();
            lblBuscar = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)numIdade).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            SuspendLayout();
             
            // txtNome
             
            txtNome.Location = new System.Drawing.Point(78, 12);
            txtNome.Name = "txtNome";
            txtNome.Size = new System.Drawing.Size(280, 23);
            txtNome.TabIndex = 3;
             
            // numIdade
             
            numIdade.Location = new System.Drawing.Point(78, 48);
            numIdade.Maximum = new decimal(new int[] { 150, 0, 0, 0 });
            numIdade.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numIdade.Name = "numIdade";
            numIdade.Size = new System.Drawing.Size(120, 23);
            numIdade.TabIndex = 4;
            numIdade.Value = new decimal(new int[] { 18, 0, 0, 0 });
             
            // btnNovo
             
            btnNovo.Location = new System.Drawing.Point(78, 85);
            btnNovo.Name = "btnNovo";
            btnNovo.Size = new System.Drawing.Size(85, 28);
            btnNovo.TabIndex = 5;
            btnNovo.Text = "Novo";
            btnNovo.Click += btnNovo_Click;
             
            // btnExcluir
             
            btnExcluir.Location = new System.Drawing.Point(169, 85);
            btnExcluir.Name = "btnExcluir";
            btnExcluir.Size = new System.Drawing.Size(85, 28);
            btnExcluir.TabIndex = 6;
            btnExcluir.Text = "Excluir";
            btnExcluir.Click += btnExcluir_Click;
             
            // btnSalvar
             
            btnSalvar.Location = new System.Drawing.Point(260, 85);
            btnSalvar.Name = "btnSalvar";
            btnSalvar.Size = new System.Drawing.Size(98, 28);
            btnSalvar.TabIndex = 7;
            btnSalvar.Text = "Salvar";
            btnSalvar.Click += btnSalvar_Click;
             
            // txtBuscar
             
            txtBuscar.Location = new System.Drawing.Point(445, 12);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new System.Drawing.Size(330, 23);
            txtBuscar.TabIndex = 8;
            txtBuscar.KeyDown += txtBuscar_KeyDown;
             
            // btnBuscar
             
            btnBuscar.Location = new System.Drawing.Point(782, 11);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new System.Drawing.Size(80, 25);
            btnBuscar.TabIndex = 9;
            btnBuscar.Text = "Buscar";
            btnBuscar.Click += btnBuscar_Click;
             
            // dgv
             
            dgv.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Location = new System.Drawing.Point(390, 48);
            dgv.MultiSelect = false;
            dgv.Name = "dgv";
            dgv.ReadOnly = true;
            dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgv.Size = new System.Drawing.Size(472, 330);
            dgv.TabIndex = 10;
            dgv.CellClick += dgv_CellClick;
             
            // lblNome
             
            lblNome.Location = new System.Drawing.Point(12, 12);
            lblNome.Name = "lblNome";
            lblNome.Size = new System.Drawing.Size(60, 23);
            lblNome.TabIndex = 2;
            lblNome.Text = "Nome:";
             
            // lblIdade
             
            lblIdade.Location = new System.Drawing.Point(12, 48);
            lblIdade.Name = "lblIdade";
            lblIdade.Size = new System.Drawing.Size(60, 23);
            lblIdade.TabIndex = 1;
            lblIdade.Text = "Idade:";
            
            // lblBuscar
             
            lblBuscar.Location = new System.Drawing.Point(392, 13);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new System.Drawing.Size(47, 23);
            lblBuscar.TabIndex = 0;
            lblBuscar.Text = "Buscar:";
            lblBuscar.Click += lblBuscar_Click;
             
            // MainForm
             
            ClientSize = new System.Drawing.Size(874, 391);
            Controls.Add(lblBuscar);
            Controls.Add(lblIdade);
            Controls.Add(lblNome);
            Controls.Add(txtNome);
            Controls.Add(numIdade);
            Controls.Add(btnNovo);
            Controls.Add(btnExcluir);
            Controls.Add(btnSalvar);
            Controls.Add(txtBuscar);
            Controls.Add(btnBuscar);
            Controls.Add(dgv);
            MinimumSize = new System.Drawing.Size(890, 430);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "CRUD C#";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)numIdade).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
