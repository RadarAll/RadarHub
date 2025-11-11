export interface EntidadeBase {
  id: number;
}

export interface RetornoOData<T> {
  items: T[];
  count: number;
}