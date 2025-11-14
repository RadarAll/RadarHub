import { CrudServicoBase } from "./CrudServicoBase"
import { Usuario } from "@/DTO/Usuario";

class UsuarioController extends CrudServicoBase<Usuario>
{
    constructor()
    {
        super("Usuario");
    }
}

const UsuarioServico = new UsuarioController();

export default UsuarioServico;