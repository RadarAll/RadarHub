import { EntidadeBase } from "./EntidadeBase";

export interface SugestaoSegmento extends EntidadeBase {
  nomeSugerido?: string;
  descricaoSugerida?: string;
  palavrasChaveSugeridas?: string;
  quantidadeLicitacoesOriginarias: number;
  confiancaPercentual: number;
  origem: TipoOrigemSugestao;
  status: StatusAprovacaoSugestao;
  dataRevisao?: Date;
  usuarioRevisao?: string;
  motivoRejeicao?: string;
  segmentoIdGerado?: number;
  licitacaoIds?: string;
}

export enum TipoOrigemSugestao {
  Clustering = 0,
  PalavrasChave = 1,
  AnaliseLDA = 2,
  Manual = 3,
  ClassificacaoDiretaLLM = 4
}

export enum StatusAprovacaoSugestao {
  Pendente = 0,
  Aprovada = 1,
  Rejeitada = 2,
  Descartada = 3
}