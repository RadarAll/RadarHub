import { TipoMargemPreferencia } from "@/DTO/TipoMargemPreferencia";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class TipoMargemPreferenciaController extends ConsultaServicoBase<TipoMargemPreferencia> {
    constructor() {
        super("TipoMargemPreferencia")
    }

    public async importar() {
        return http.post(`/TipoMargemPreferencia/importar`);
    }
}

const TipoMargemPreferenciaServico = new TipoMargemPreferenciaController()

export default TipoMargemPreferenciaServico;