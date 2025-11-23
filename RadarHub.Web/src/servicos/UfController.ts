import { Uf } from "@/DTO/Uf";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class UfController extends ConsultaServicoBase<Uf> {
    constructor() {
        super("Ufs")
    }

    public async importar() {
        return http.post(`/Ufs/importar`);
    }
}

const UfServico = new UfController();

export default UfServico;