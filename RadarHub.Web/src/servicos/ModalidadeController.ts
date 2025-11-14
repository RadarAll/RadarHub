import { Modalidade } from "../DTO/Modalidade";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class ModalidadeController extends ConsultaServicoBase<Modalidade>{
    constructor(){
        super("Modalidade") 
    }

    public async importar() {
        return http.post(`/modalidade/importar`)
    }
    
}

const ModalidadeServico = new ModalidadeController();

export default ModalidadeServico;