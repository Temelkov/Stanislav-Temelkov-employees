import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result } from '../models/result.model';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  private apiUrl = 'https://localhost:44352/EmployeeProjects/upload';

  constructor(private http: HttpClient) {}

  uploadFile(file: File): Observable<Result[]> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<Result[]>(this.apiUrl, formData);
  }
}
