import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-port',
  templateUrl: './port.component.html',
  styleUrls: ['./port.component.scss'],
})
export class PortComponent implements OnInit {
  @Input() public top?: number;
  @Input() public left?: number;

  constructor() {}
  ngOnInit(): void {}
}
