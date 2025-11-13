import { FonteOrcamentaria } from "../DTO/FonteOrcamentaria";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon"

class FonteOrcamentariaServico extends ConsultaServicoBase<FonteOrcamentaria> {
    constructor() {
        super("FonteOrcamentaria")
    }

    public async importar() {
        return http.post(`/fonteorcamentaria/importar`)
    }
}

export default new FonteOrcamentariaServico();