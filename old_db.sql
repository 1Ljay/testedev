-- db.sql — Banco: TesteDev (nome + idade)
\connect "TesteDev";

CREATE TABLE IF NOT EXISTS public.cadastro (
  id      INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  nome    TEXT    NOT NULL,
  idade   INTEGER NOT NULL,
  CONSTRAINT ck_idade_pos CHECK (idade > 0),
  CONSTRAINT uq_idade UNIQUE (idade)
);

CREATE TABLE IF NOT EXISTS public.log_operacoes (
  id            BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  cadastro_id   INTEGER,
  operacao      TEXT NOT NULL CHECK (operacao IN ('INSERT','UPDATE','DELETE')),
  instante      TIMESTAMPTZ NOT NULL DEFAULT now()
);

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
    INSERT INTO public.log_operacoes (cadastro_id, operacao) VALUES (OLD.id, 'DELETE');
    RETURN OLD;
  END IF;
  RETURN NULL;
END;
$$;

DROP TRIGGER IF EXISTS trg_cadastro_log ON public.cadastro;
CREATE TRIGGER trg_cadastro_log
AFTER INSERT OR UPDATE OR DELETE ON public.cadastro
FOR EACH ROW EXECUTE FUNCTION public.trg_log_operacoes();

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
REVOKE ALL ON TABLE public.log_operacoes FROM app_user;

INSERT INTO public.cadastro (nome, idade) VALUES ('João Silva', 25);
INSERT INTO public.cadastro (nome, idade) VALUES ('Maria Souza', 30);
