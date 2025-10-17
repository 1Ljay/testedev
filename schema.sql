-- schema.sql — Banco: TesteDev
-- Este script cria o esquema necessário para o aplicativo WinForms (CRUD de cadastro).
-- Compatível com PostgreSQL 13+ e executável no pgAdmin (Query Tool).

-- 1) (Opcional) Criar a base se necessário
-- CREATE DATABASE "TesteDev" WITH ENCODING='UTF8' TEMPLATE=template0 LC_COLLATE='en_US.UTF-8' LC_CTYPE='en_US.UTF-8';

-- 2) Conectar na base (no pgAdmin, selecione a base pela UI ou use o comando abaixo)
\connect "TesteDev";

-- 3) Tabelas principais

-- Tabela: cadastro
-- Armazena os registros de pessoas com nome e idade.
CREATE TABLE IF NOT EXISTS public.cadastro (
	id		INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	nome	TEXT    NOT NULL,
	idade	INTEGER,
	CONSTRAINT ck_idade_pos CHECK (idade IS NULL OR idade > 0),
	CONSTRAINT ck_nome_not_blank CHECK (btrim(nome) <> ''),
	CONSTRAINT uq_idade UNIQUE (idade)
);

-- Índices adicionais úteis (opcional)
-- Index para acelerar buscas por nome (ILIKE/LIKE). Para unaccent, a aplicação já faz fallback.
CREATE INDEX IF NOT EXISTS idx_cadastro_nome ON public.cadastro (lower(nome));

-- 4) Tabela de logs (audit trail simples)
-- Registra operações DML realizadas na tabela cadastro.
CREATE TABLE IF NOT EXISTS public.log_operacoes (
	id			BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	cadastro_id	INTEGER,
	operacao	TEXT NOT NULL CHECK (operacao IN ('INSERT','UPDATE','DELETE')),
	instante	TIMESTAMPTZ NOT NULL DEFAULT now(),
	CONSTRAINT fk_log_cadastro FOREIGN KEY (cadastro_id) REFERENCES public.cadastro(id) ON DELETE SET NULL
);

-- 5) Trigger de log
CREATE OR REPLACE FUNCTION public.trg_log_operacoes()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
	IF TG_OP = 'INSERT' THEN
		INSERT INTO public.log_operacoes (cadastro_id, operacao) VALUES (NEW.id, 'INSERT');
		RETURN NEW;
	ELSIF TG_OP = 'UPDATE' THEN
		INSERT INTO public.log_operacoes (cadastro_id, operacao) VALUES (NEW.id, 'UPDATE');
		RETURN NEW;
	ELSIF TG_OP = 'DELETE' THEN
		INSERT INTO public.log_operacoes (OLD.id, 'DELETE');
		RETURN OLD;
	END IF;
	RETURN NULL;
END;
$$;

DROP TRIGGER IF EXISTS trg_cadastro_log ON public.cadastro;
CREATE TRIGGER trg_cadastro_log
AFTER INSERT OR UPDATE OR DELETE ON public.cadastro
FOR EACH ROW EXECUTE FUNCTION public.trg_log_operacoes();

-- 6) Usuário de aplicação e permissões
DO $$
BEGIN
	IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'app_user') THEN
		CREATE ROLE app_user LOGIN PASSWORD 'SenhaForte!123';
	ELSE
		ALTER ROLE app_user WITH LOGIN PASSWORD 'SenhaForte!123';
	END IF;
END;
$$;

REVOKE ALL ON SCHEMA public FROM PUBLIC;
GRANT USAGE ON SCHEMA public TO app_user;

REVOKE ALL ON ALL TABLES IN SCHEMA public FROM PUBLIC;
REVOKE ALL ON ALL TABLES IN SCHEMA public FROM app_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE public.cadastro TO app_user;
REVOKE ALL ON TABLE public.log_operacoes FROM app_user; -- log apenas interno

-- 7) Dados iniciais (opcional, úteis para validar a UI)
INSERT INTO public.cadastro (nome, idade) VALUES ('João Silva', 25) ON CONFLICT DO NOTHING;
INSERT INTO public.cadastro (nome, idade) VALUES ('Maria Souza', 30) ON CONFLICT DO NOTHING;
