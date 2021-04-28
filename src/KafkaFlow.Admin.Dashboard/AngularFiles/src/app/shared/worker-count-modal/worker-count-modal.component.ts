import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-worker-count-modal',
  templateUrl: './worker-count-modal.component.html',
  styleUrls: ['./worker-count-modal.component.css']
})
export class WorkerCountModalComponent implements OnInit {
  @Input() public workerCount: any;
  @Input() public groupId: any;
  @Input() public consumerName: any;
  public oldWorkerCount: any;  
  
  constructor(public activeModal: NgbActiveModal) {}

  ngOnInit(): void {        
    this.oldWorkerCount = this.workerCount;
  }

  save() {
    this.activeModal.close(this.workerCount);
  }
}
