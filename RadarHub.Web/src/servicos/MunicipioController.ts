import { Municipio } from "@/DTO/Municipio";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class MunicipioController extends ConsultaServicoBase<Municipio> {
    constructor() {
        super("Municipio")
    }

    public async importar() {
        return http.post(`/municipio/importar`)
    }
}

const MunicipioServico = new MunicipioController();

export default MunicipioServico;
