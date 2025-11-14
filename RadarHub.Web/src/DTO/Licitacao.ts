import { EntidadeBase } from "./EntidadeBase";

export interface Licitacao extends EntidadeBase {
  titulo: string;
  descricao: string;
  itemUrl: string;
  valorGlobal?: number;
  temResultado: boolean;

  dataPublicacaoPncp?: Date;
  dataAtualizacaoPncp?: Date;
  dataInicioVigencia?: Date;
  dataFimVigencia?: Date;

  ano: string;
  numeroSequencial?: string;
  numeroSequencialCompraAta?: string;
  numeroControlePncp?: string;

  orgaoIdTerceiro?: string;
  municipioIdTerceiro?: string;
  modalidadeIdTerceiro?: string;
  tipoIdTerceiro?: string;
  ufIdTerceiro?: string;
  fonteOrcamentariaIdTerceiro?: string;
  tipoMargemPreferenciaIdTerceiro?: string;
  unidadeIdTerceiro?: string;
  poderIdTerceiro?: string;
  esferaIdTerceiro?: string;

  orgaoSubrogadoId?: string;
  orgaoSubrogadoNome?: string;

  exigenciaConteudoNacional: boolean;
}