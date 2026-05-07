import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Tarefa, CreateTarefaDto, UpdateTarefaDto } from '../models/tarefa.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TarefaService {
  private readonly apiUrl = `${environment.apiUrl}/tarefas`;

  constructor(private http: HttpClient) {}

  getAll(status?: string): Observable<Tarefa[]> {
    let params = new HttpParams();
    if (status) params = params.set('status', status);
    return this.http.get<Tarefa[]>(this.apiUrl, { params }).pipe(catchError(this.handleError));
  }

  getById(id: number): Observable<Tarefa> {
    return this.http.get<Tarefa>(`${this.apiUrl}/${id}`).pipe(catchError(this.handleError));
  }

  create(dto: CreateTarefaDto): Observable<Tarefa> {
    return this.http.post<Tarefa>(this.apiUrl, dto).pipe(catchError(this.handleError));
  }

  update(id: number, dto: UpdateTarefaDto): Observable<Tarefa> {
    return this.http.put<Tarefa>(`${this.apiUrl}/${id}`, dto).pipe(catchError(this.handleError));
  }

  updateStatus(id: number, status: string): Observable<Tarefa> {
    return this.http.patch<Tarefa>(`${this.apiUrl}/${id}/status`, { status }).pipe(catchError(this.handleError));
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    const message =
      error.error?.message ??
      (error.status === 0
        ? 'Não foi possível conectar ao servidor.'
        : `Erro ${error.status}: ${error.statusText}`);

    return throwError(() => new Error(message));
  }
}
