import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TarefaService } from '../../services/tarefa.service';
import { Tarefa, CreateTarefaDto, StatusTarefa } from '../../models/tarefa.model';
import { TaskCardComponent } from '../task-card/task-card.component';
import { TaskModalComponent } from '../task-modal/task-modal.component';

interface Coluna {
  status: StatusTarefa;
  label: string;
  cor: string;
  icone: string;
}

@Component({
  selector: 'app-kanban-board',
  standalone: true,
  imports: [CommonModule, FormsModule, TaskCardComponent, TaskModalComponent],
  templateUrl: './kanban-board.component.html',
  styleUrls: ['./kanban-board.component.scss']
})
export class KanbanBoardComponent implements OnInit {
  tarefas: Tarefa[] = [];
  modalAberto = false;
  tarefaEditando: Tarefa | null = null;
  filtroPrioridade = '';
  mensagem: { texto: string; tipo: 'sucesso' | 'erro' } | null = null;
  private toastTimer: ReturnType<typeof setTimeout> | null = null;

  readonly colunas: Coluna[] = [
    { status: 'Pendente',     label: 'Pendente',     cor: '#f59e0b', icone: 'schedule' },
    { status: 'Em Andamento', label: 'Em Andamento', cor: '#6366f1', icone: 'autorenew' },
    { status: 'Concluída',    label: 'Concluída',    cor: '#22c55e', icone: 'check_circle' }
  ];

  constructor(private tarefaService: TarefaService) {}

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.tarefaService.getAll().subscribe({
      next: t => this.tarefas = t,
      error: (e: Error) => this.mostrarMensagem(e.message, 'erro')
    });
  }

  tarefasPorStatus(status: StatusTarefa): Tarefa[] {
    return this.tarefas
      .filter(t => t.status === status && (!this.filtroPrioridade || t.prioridade === this.filtroPrioridade));
  }

  abrirModal(tarefa: Tarefa | null = null): void {
    this.tarefaEditando = tarefa;
    this.modalAberto = true;
  }

  fecharModal(): void {
    this.modalAberto = false;
    this.tarefaEditando = null;
  }

  salvar(dto: CreateTarefaDto): void {
    if (this.tarefaEditando) {
      this.tarefaService.update(this.tarefaEditando.id, dto).subscribe({
        next: atualizada => {
          this.tarefas = this.tarefas.map(t => t.id === atualizada.id ? atualizada : t);
          this.mostrarMensagem('Tarefa atualizada com sucesso.', 'sucesso');
          this.fecharModal();
        },
        error: (e: Error) => this.mostrarMensagem(e.message, 'erro')
      });
    } else {
      this.tarefaService.create(dto).subscribe({
        next: nova => {
          this.tarefas = [nova, ...this.tarefas];
          this.mostrarMensagem('Tarefa criada com sucesso.', 'sucesso');
          this.fecharModal();
        },
        error: (e: Error) => this.mostrarMensagem(e.message, 'erro')
      });
    }
  }

  excluir(id: number): void {
    if (!confirm('Deseja excluir esta tarefa?')) return;
    this.tarefaService.delete(id).subscribe({
      next: () => {
        this.tarefas = this.tarefas.filter(t => t.id !== id);
        this.mostrarMensagem('Tarefa excluída com sucesso.', 'sucesso');
      },
      error: (e: Error) => this.mostrarMensagem(e.message, 'erro')
    });
  }

  moverTarefa(evento: { id: number; status: string }): void {
    this.tarefaService.updateStatus(evento.id, evento.status).subscribe({
      next: atualizada => {
        this.tarefas = this.tarefas.map(t => t.id === atualizada.id ? atualizada : t);
        this.mostrarMensagem(`Tarefa movida para "${evento.status}".`, 'sucesso');
      },
      error: (e: Error) => this.mostrarMensagem(e.message, 'erro')
    });
  }

  private mostrarMensagem(texto: string, tipo: 'sucesso' | 'erro'): void {
    if (this.toastTimer) clearTimeout(this.toastTimer);
    this.mensagem = { texto, tipo };
    this.toastTimer = setTimeout(() => this.mensagem = null, 3500);
  }

  get totalTarefas(): number { return this.tarefas.length; }
  get tarefasConcluidas(): number { return this.tarefas.filter(t => t.status === 'Concluída').length; }
}
