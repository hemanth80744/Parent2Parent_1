import { DatePipe } from '@angular/common';
import { Component, ElementRef, effect, input, output, viewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChatMessage } from '../../../services/models';

@Component({
  selector: 'app-chat-box',
  standalone: true,
  imports: [FormsModule, DatePipe],
  templateUrl: './chat-box.component.html',
  styleUrl: './chat-box.component.css'
})
export class ChatBoxComponent {
  readonly meId = input.required<number>();
  readonly otherName = input<string>('Chat');
  readonly messages = input<ChatMessage[]>([]);

  readonly send = output<string>();

  draft = '';

  private readonly scroller = viewChild<ElementRef>('scroller');

  constructor() {
    effect(() => {
      // Auto-scroll to bottom when messages change
      this.messages();
      setTimeout(() => this.scrollToBottom(), 10);
    });
  }

  onSend() {
    const t = this.draft.trim();
    if (!t) return;
    this.send.emit(t);
    this.draft = '';
  }

  private scrollToBottom() {
    const el = this.scroller()?.nativeElement;
    if (el) {
      el.scrollTop = el.scrollHeight;
    }
  }

  trackById = (_: number, m: ChatMessage) => m.id;
}

