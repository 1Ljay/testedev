param(
  [string]$DbHost = 'localhost',
  [int]$Port = 5432,
  [string]$SuperUser = 'postgres',
  [string]$Db = 'TesteDev',
  [string]$Password = ''
)

$psql = 'C:\Program Files\PostgreSQL\18\bin\psql.exe'
$scriptPath = 'C:\Users\Ljay\Desktop\teste\db.sql'

function Invoke-PSQL {
  param([string[]]$Args)
  $out = & $psql @Args 2>&1
  $code = $LASTEXITCODE
  if ($code -ne 0) {
    throw ('Falha ao executar: psql ' + ($Args -join ' ') + "`n" + $out)
  }
  return $out
}

# --- Validações ---
if (-not (Test-Path $psql)) { throw 'psql.exe não encontrado em: ' + $psql }
if (-not (Test-Path $scriptPath)) { throw 'db.sql não encontrado em: ' + $scriptPath }

# --- Forçar variáveis de ambiente para o psql ---
# Remove qualquer PGUSER antigo e define explicitamente o superusuário
Remove-Item Env:\PGUSER -ErrorAction SilentlyContinue
$env:PGUSER = $SuperUser

# Senha via env var (se fornecida)
if ($Password -ne '') { $env:PGPASSWORD = $Password }

Write-Host ('>> Verificando existência do banco ''' + $Db + '''...') -ForegroundColor Cyan

# Use sempre a forma longa --username= para evitar qualquer ambiguidade
$exists = Invoke-PSQL -Args @(
  '-h', $DbHost,
  '-p', $Port.ToString(),
  '--username=' + $SuperUser,
  '-d', 'postgres',
  '-t', '-A',
  '-c', "SELECT 1 FROM pg_database WHERE datname='$Db';"
)

if ($exists.Trim() -ne '1') {
  Write-Host ('>> Criando banco ''' + $Db + '''...') -ForegroundColor Yellow
  Invoke-PSQL -Args @(
    '-h', $DbHost,
    '-p', $Port.ToString(),
    '--username=' + $SuperUser,
    '-d', 'postgres',
    '-c', ('CREATE DATABASE "' + $Db + '" WITH ENCODING ''UTF8'' TEMPLATE template1;')
  )
} else {
  Write-Host ('>> Banco ''' + $Db + ''' já existe.') -ForegroundColor Green
}

Write-Host ('>> Executando db.sql em ''' + $Db + '''...') -ForegroundColor Cyan
Invoke-PSQL -Args @(
  '-h', $DbHost,
  '-p', $Port.ToString(),
  '--username=' + $SuperUser,
  '-d', $Db,
  '-f', $scriptPath
)

# Limpa variáveis
Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
Remove-Item Env:\PGUSER -ErrorAction SilentlyContinue

Write-Host ('✔ Banco ''' + $Db + ''' pronto!') -ForegroundColor Green
Write-Host ('   - Script executado: ' + $scriptPath)
Write-Host ('   - psql: ' + $psql)
