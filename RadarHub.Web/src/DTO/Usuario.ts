import { EntidadeBase } from "./EntidadeBase";

export interface Usuario extends EntidadeBase {
  nomeCompleto: string;
  email: string;
  senhaHash: string;
  ativo: boolean;
  dataDesativacao?: Date;
}