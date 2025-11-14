import { FonteOrcamentaria } from "../DTO/FonteOrcamentaria";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon"

class FonteOrcamentariaController extends ConsultaServicoBase<FonteOrcamentaria> {
    constructor() {
        super("FonteOrcamentaria")
    }

    public async importar() {
        return http.post(`/fonteorcamentaria/importar`)
    }
}

const FonteOrcamentariaServico = new FonteOrcamentariaController()

export default FonteOrcamentariaServico;