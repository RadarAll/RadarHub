import { Unidade } from "@/DTO/Unidade";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class UnidadeController extends ConsultaServicoBase<Unidade> {
    constructor() {
        super("Unidade")
    }

    public async importar() {
        return http.post(`/unidade/importar`);
    }
}

const UnidadeServico = new UnidadeController();

export default UnidadeServico;