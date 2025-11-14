import { Licitacao } from "@/DTO/Licitacao";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class LicitacaoController extends ConsultaServicoBase<Licitacao>
{
    constructor()
    {
        super("Licitacao")
    }

    public async importar()
    {
        return http.post(`/licitacao/importar`);
    }
}

const LicitacaoServico = new LicitacaoController()

export default LicitacaoServico;