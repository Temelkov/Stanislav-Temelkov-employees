import { Component } from '@angular/core';
import { Result } from 'src/app/models/result.model';
import { FileUploadService } from 'src/app/services/file-upload.service';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.scss']
})
export class FileUploadComponent {
  selectedFile: File | null = null;
  result: Result[] = [];
  error: string | null = null;
  displayedColumns: string[] = ['empID1', 'empID2', 'projectID', 'daysWorked'];

  constructor(private fileUploadService: FileUploadService) {}

  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0];
  }

  onUpload(): void {
    if (!this.selectedFile) return;

    const formData = new FormData();
    formData.append('file', this.selectedFile);

    this.fileUploadService.uploadFile(this.selectedFile)
      .subscribe({
        next: (response) => {
          this.result = response;
          this.error = null;
        },
        error: (err) => {
          this.error = 'An error occurred while uploading the file.';
          this.result = [];
        }
      });
  }
}
