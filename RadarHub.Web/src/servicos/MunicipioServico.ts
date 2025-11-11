import { Municipio } from "@/DTO/Municipio";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class MunicipioServico extends ConsultaServicoBase<Municipio> {
    constructor() {
        super("Municipio")
    }

    public async importar() {
        return http.post(`/municipio/importar`)
    }
}

export default new MunicipioServico();
