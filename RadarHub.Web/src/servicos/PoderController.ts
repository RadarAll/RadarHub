import { Poder } from "@/DTO/Poder";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class PoderController extends ConsultaServicoBase<Poder> {
    constructor() {
        super("Poder");
    }

    public async importar() {
        return http.post(`/poder/importar`);
    }
}

const PoderServico = new PoderController()

export default PoderServico;