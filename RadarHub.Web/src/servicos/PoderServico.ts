import { Poder } from "@/DTO/Poder";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class PoderServico extends ConsultaServicoBase<Poder> {
    constructor() {
        super("Poder");
    }

    public async importar() {
        return http.post(`/poder/importar`);
    }
}

export default new PoderServico();