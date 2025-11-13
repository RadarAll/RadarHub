import { Orgao } from "@/DTO/Orgao";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class OrgaoServico extends ConsultaServicoBase<Orgao> {
    constructor() {
        super("Orgao");
    }

    public async importar() {
        return http.post(`/orgao/importar`)
    }
}

export default new OrgaoServico();