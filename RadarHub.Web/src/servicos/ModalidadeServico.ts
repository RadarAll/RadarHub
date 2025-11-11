import { Modalidade } from "../DTO/Modalidade";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class ModalidadeServico extends ConsultaServicoBase<Modalidade>{
    constructor(){
        super("Modalidade") 
    }

    public async importar() {
        return http.post(`/modalidade/importar`)
    }
    
}

export default new ModalidadeServico();