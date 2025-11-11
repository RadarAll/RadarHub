import { TipoMargemPreferencia } from "@/DTO/TipoMargemPreferencia";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class TipoMargemPreferenciaServico extends ConsultaServicoBase<TipoMargemPreferencia> {
    constructor() {
        super("TipoMargePreferencia")
    }

    public async importar() {
        return http.post(`/tipomargempreferencia/importar`);
    }
}

export default new TipoMargemPreferenciaServico();