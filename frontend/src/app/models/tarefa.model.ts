export type StatusTarefa = 'Pendente' | 'Em Andamento' | 'Concluída';
export type PrioridadeTarefa = 'Baixa' | 'Média' | 'Alta';

export interface Tarefa {
  id: number;
  titulo: string;
  descricao: string;
  status: StatusTarefa;
  prioridade: PrioridadeTarefa;
  dataCriacao: string;
  dataAtualizacao?: string;
}

export interface CreateTarefaDto {
  titulo: string;
  descricao: string;
  status: StatusTarefa;
  prioridade: PrioridadeTarefa;
}

export interface UpdateTarefaDto extends CreateTarefaDto {}
