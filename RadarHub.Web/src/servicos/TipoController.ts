import { Tipo } from "@/DTO/Tipo";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class TipoController extends ConsultaServicoBase<Tipo> {
    constructor() {
        super("Tipo")
    }

    public async importar() {
        return http.post(`/tipo/importar`)
    }
}

const TipoServico = new TipoController();

export default TipoServico;