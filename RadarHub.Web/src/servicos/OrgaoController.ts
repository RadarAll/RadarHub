import { Orgao } from "@/DTO/Orgao";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class OrgaoController extends ConsultaServicoBase<Orgao> {
    constructor() {
        super("Orgao");
    }

    public async importar() {
        return http.post(`/orgao/importar`)
    }
}

const OrgaoServico = new OrgaoController();

export default OrgaoServico;