import { EntidadeBase } from "./EntidadeBase";

export interface UsuarioBase extends EntidadeBase {
  nomeCompleto: string;
  email: string;
  senhaHash: string;
  ativo: boolean;
  dataDesativacao?: Date;
}