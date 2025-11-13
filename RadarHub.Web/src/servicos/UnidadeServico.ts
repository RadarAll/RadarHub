import { Unidade } from "@/DTO/Unidade";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class UnidadeServico extends ConsultaServicoBase<Unidade> {
    constructor() {
        super("Unidade")
    }

    public async importar() {
        return http.post(`/unidade/importar`);
    }
}

export default new UnidadeServico();