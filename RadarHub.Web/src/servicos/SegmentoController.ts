import { Segmento } from "@/DTO/Segmento";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class SegmentoController extends ConsultaServicoBase<Segmento> {
    constructor() {
        super("Segmento");
    }
}

const SegmentoServico = new SegmentoController()

export default SegmentoServico;