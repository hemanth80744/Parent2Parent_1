import { Component, input, output } from '@angular/core';
import { ParentProfile } from '../../../services/models';

@Component({
  selector: 'app-chat-users',
  standalone: true,
  templateUrl: './chat-users.component.html',
  styleUrl: './chat-users.component.css'
})
export class ChatUsersComponent {
  readonly users = input<ParentProfile[]>([]);
  readonly selectedId = input<number | null>(null);
  readonly pick = output<number>();

  select(id: number) {
    this.pick.emit(id);
  }
}

