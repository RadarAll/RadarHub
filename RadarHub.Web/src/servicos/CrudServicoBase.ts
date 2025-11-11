// src/services/CrudServiceBase.ts

import http from "./http-comon";
import { AxiosResponse } from "axios";
import { ConsultaServicoBase } from "./ConsultaServicoBase";
import { EntidadeBase } from "@/dtos/EntidadeBase";

// Definição para o corpo da resposta padronizada da sua API
interface ApiResponse<T> {
    mensagens: { Texto: string, Tipo: string }[];
    dados?: T;
}

/**
 * Serviço base para operações de CRUD (Create, Read, Update, Delete)
 * @param TEntity O tipo da Entidade (ex: Usuario, Produto) que deve estender EntidadeBase.
 */
export class CrudServicoBase<TEntity extends EntidadeBase> extends ConsultaServicoBase<TEntity> {
  // Construtor já chama o construtor da classe pai (ConsultaServicoBase)
  constructor(nomeBaseDaRota: string) {
    super(nomeBaseDaRota);
  }

  /**
   * Adiciona uma nova entidade. (POST /api/{nomeBaseDaRota})
   */
  public async adicionar(entidade: Omit<TEntity, 'id'>): Promise<AxiosResponse<ApiResponse<TEntity>>> {
    return http.post<ApiResponse<TEntity>>(`/${this.baseUrl}`, entidade);
  }

  /**
   * Atualiza uma entidade existente (PUT /api/{nomeBaseDaRota})
   */
  public async atualizar(entidade: TEntity): Promise<AxiosResponse<ApiResponse<TEntity>>> {
    return http.put<ApiResponse<TEntity>>(`/${this.baseUrl}`, entidade);
  }

  /**
   * Atualiza parcialmente uma entidade (PATCH /api/{nomeBaseDaRota}/{id})
   * O 'patchDoc' deve seguir a especificação JSON Patch.
   */
  public async atualizarParcialmente(
    id: number | string,
    patchDoc: any // Tipo JsonPatchDocument<TEntity> do C#
  ): Promise<AxiosResponse<ApiResponse<TEntity>>> {
    return http.patch<ApiResponse<TEntity>>(`/${this.baseUrl}/${id}`, patchDoc);
  }

  /**
   * Remove uma entidade pelo ID. (DELETE /api/{nomeBaseDaRota}/{id})
   */
  public async remover(id: number | string): Promise<AxiosResponse<ApiResponse<TEntity>>> {
    return http.delete<ApiResponse<TEntity>>(`/${this.baseUrl}/${id}`);
  }
}