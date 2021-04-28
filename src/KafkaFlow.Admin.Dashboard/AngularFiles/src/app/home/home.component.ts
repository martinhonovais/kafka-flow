import { Component, OnInit } from '@angular/core';
import { ConsumerService } from '../consumer.service'

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  public groups: Array<any> = [];
  constructor (private consumerService: ConsumerService) {
    consumerService.get().subscribe((data: any) => this.groups = data);    
  }

  ngOnInit(): void {
  }

}
