import { Uf } from "@/DTO/Uf";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class UfServico extends ConsultaServicoBase<Uf> {
    constructor() {
        super("Uf")
    }

    public async importar() {
        return http.post(`/uf/importar`);
    }
}

export default new UfServico();