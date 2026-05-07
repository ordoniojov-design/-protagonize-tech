import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Tarefa } from '../../models/tarefa.model';

@Component({
  selector: 'app-task-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './task-card.component.html',
  styleUrls: ['./task-card.component.scss']
})
export class TaskCardComponent {
  @Input() tarefa!: Tarefa;
  @Output() editar = new EventEmitter<Tarefa>();
  @Output() excluir = new EventEmitter<number>();
  @Output() moverPara = new EventEmitter<{ id: number; status: string }>();

  readonly statusFlow: Record<string, string[]> = {
    'Pendente': ['Em Andamento'],
    'Em Andamento': ['Pendente', 'Concluída'],
    'Concluída': ['Em Andamento']
  };

  get proximosStatus(): string[] {
    return this.statusFlow[this.tarefa.status] ?? [];
  }

  get prioridadeClass(): string {
    return {
      'Alta': 'prioridade-alta',
      'Média': 'prioridade-media',
      'Baixa': 'prioridade-baixa'
    }[this.tarefa.prioridade] ?? '';
  }

  onMover(status: string): void {
    this.moverPara.emit({ id: this.tarefa.id, status });
  }
}
