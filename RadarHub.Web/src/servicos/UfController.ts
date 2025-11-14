import { Uf } from "@/DTO/Uf";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class UfController extends ConsultaServicoBase<Uf> {
    constructor() {
        super("Uf")
    }

    public async importar() {
        return http.post(`/uf/importar`);
    }
}

const UfServico = new UfController();

export default UfServico;