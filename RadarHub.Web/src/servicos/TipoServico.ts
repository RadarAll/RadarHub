import { Tipo } from "@/DTO/Tipo";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class TipoServico extends ConsultaServicoBase<Tipo> {
    constructor() {
        super("Tipo")
    }

    public async importar() {
        return http.post(`/tipo/importar`)
    }
}

export default new TipoServico();