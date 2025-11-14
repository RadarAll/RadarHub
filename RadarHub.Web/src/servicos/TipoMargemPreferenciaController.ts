import { TipoMargemPreferencia } from "@/DTO/TipoMargemPreferencia";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class TipoMargemPreferenciaController extends ConsultaServicoBase<TipoMargemPreferencia> {
    constructor() {
        super("TipoMargePreferencia")
    }

    public async importar() {
        return http.post(`/tipomargempreferencia/importar`);
    }
}

const TipoMargemPreferenciaServico = new TipoMargemPreferenciaController()

export default TipoMargemPreferenciaServico;