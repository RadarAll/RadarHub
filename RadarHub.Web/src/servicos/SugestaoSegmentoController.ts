import { SugestaoSegmento } from "@/DTO/SugestaoSegmento";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import http from "./http-comon";

class SugestaoSegmentoController extends ConsultaServicoBase<SugestaoSegmento> {
    constructor() {
        super("SugestaoSegmento")
    }
}

const SugetaoSegmentoService = new SugestaoSegmentoController();

export default SugetaoSegmentoService;