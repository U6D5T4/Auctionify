import { formatDate } from '@angular/common';
import {
    Component,
    ElementRef,
    EventEmitter,
    Inject,
    Input,
    LOCALE_ID,
    OnChanges,
    OnInit,
    Output,
    SimpleChanges,
    ViewChild,
    effect,
} from '@angular/core';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { ChatMessage, Conversation } from 'src/app/models/chats/chat-models';
import { SignalRService } from 'src/app/services/signalr-service/signalr.service';
import { Client } from 'src/app/web-api-client';
import { ChatsComponent } from '../chats/chats.component';

interface GroupedMessages {
    date: string;
    messages: ChatMessage[];
}

@Component({
    selector: 'app-conversation-window',
    templateUrl: './conversation-window.component.html',
    styleUrls: ['./conversation-window.component.scss'],
})
export class ConversationWindowComponent implements OnInit, OnChanges {
    @Input() conversation!: Conversation;
    currentUserId: number = 0;
    messages: ChatMessage[] = [];
    private isSignalrConnected = false;
    @ViewChild('msg_input') input!: ElementRef<HTMLInputElement>;

    @Output() messageSent = new EventEmitter<boolean>();

    constructor(
        private authService: AuthorizeService,
        private client: Client,
        private signalRService: SignalRService,
        @Inject(LOCALE_ID) public locale: string
    ) {
        effect(() => {
            this.currentUserId = this.authService.getUserId()!;
        });
    }
    ngOnChanges(changes: SimpleChanges): void {
        this.ngOnInit();
    }

    async ngOnInit(): Promise<void> {
        this.getAllConversationMessages();

        if (!this.isSignalrConnected) {
            await this.signalRService.joinConversationGroup(
                this.conversation.id
            );

            this.signalRService.onReceiveChatMessage(() => {
                this.getAllConversationMessages();
                this.messageSent.emit(true);
            }, this.conversation.id);
        }
    }

    getAllConversationMessages() {
        this.client
            .getAllConversationChatMessages(this.conversation.id)
            .subscribe({
                next: (result) => {
                    result.chatMessages.map(
                        (el) => (el.timeStamp = new Date(el.timeStamp))
                    );
                    this.messages = result.chatMessages;
                },
            });
    }

    groupMessagesByDate(messages: ChatMessage[]): GroupedMessages[] {
        const groupedMessages: GroupedMessages[] = [];
        const today = new Date();

        messages.forEach((message) => {
            let dateKey = formatDate(
                message.timeStamp,
                'longDate',
                this.locale
            );

            if (message.timeStamp.getDate() == today.getDate()) {
                dateKey = 'Today';
            }

            const existingGroup = groupedMessages.find(
                (group) => group.date === dateKey
            );

            if (existingGroup) {
                existingGroup.messages.push(message);
            } else {
                groupedMessages.push({ date: dateKey, messages: [message] });
            }
        });

        groupedMessages.sort(
            (a, b) => new Date(a.date).getTime() - new Date(b.date).getTime()
        );

        return groupedMessages;
    }

    sendMessage(input: string) {
        this.client.sendChatMessage(this.conversation.id, input).subscribe({
            next: () => {
                this.input.nativeElement.value = '';
                console.log(this.input.nativeElement);
            },
        });
    }

    generateMessageClass(message: ChatMessage) {
        if (this.currentUserId == message.senderId) {
            return 'self';
        } else return 'other';
    }
}
