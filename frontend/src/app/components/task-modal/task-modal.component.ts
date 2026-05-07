import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Tarefa, CreateTarefaDto } from '../../models/tarefa.model';

@Component({
  selector: 'app-task-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './task-modal.component.html',
  styleUrls: ['./task-modal.component.scss']
})
export class TaskModalComponent implements OnInit {
  @Input() tarefa: Tarefa | null = null;
  @Output() salvar = new EventEmitter<CreateTarefaDto>();
  @Output() fechar = new EventEmitter<void>();

  form!: FormGroup;

  readonly statusOptions = ['Pendente', 'Em Andamento', 'Concluída'];
  readonly prioridadeOptions = ['Baixa', 'Média', 'Alta'];

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      titulo: [this.tarefa?.titulo ?? '', [Validators.required, Validators.maxLength(200)]],
      descricao: [this.tarefa?.descricao ?? '', Validators.maxLength(1000)],
      status: [this.tarefa?.status ?? 'Pendente', Validators.required],
      prioridade: [this.tarefa?.prioridade ?? 'Média', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.salvar.emit(this.form.value);
  }

  get titulo() { return this.form.get('titulo'); }
}
