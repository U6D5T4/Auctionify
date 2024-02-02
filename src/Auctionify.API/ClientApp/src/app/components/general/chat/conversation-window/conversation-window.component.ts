import { formatDate } from '@angular/common';
import {
    AfterViewInit,
    Component,
    ElementRef,
    EventEmitter,
    Inject,
    Input,
    LOCALE_ID,
    OnChanges,
    OnInit,
    Output,
    QueryList,
    SimpleChanges,
    ViewChild,
    ViewChildren,
    effect,
} from '@angular/core';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { ChatMessage, Conversation } from 'src/app/models/chats/chat-models';
import { SignalRService } from 'src/app/services/signalr-service/signalr.service';
import { Client } from 'src/app/web-api-client';
import { ChatsComponent } from '../chats/chats.component';
import { Observable, last } from 'rxjs';
import {
    IntersectResult,
    MessageObserverDirective,
} from 'src/app/directives/message-observer.directive';

interface GroupedMessages {
    date: string;
    messages: ChatMessage[];
}

@Component({
    selector: 'app-conversation-window',
    templateUrl: './conversation-window.component.html',
    styleUrls: ['./conversation-window.component.scss'],
})
export class ConversationWindowComponent
    implements OnInit, AfterViewInit, OnChanges
{
    @Input() conversation!: Conversation;
    @Input() updateChatEvent!: Observable<void>;
    currentUserId: number = 0;
    messages: GroupedMessages[] = [];
    @ViewChild('msg_input') input!: ElementRef<HTMLInputElement>;
    @ViewChild('main_conversation')
    mainConversationWindow!: ElementRef<HTMLElement>;
    @ViewChildren('messagePool')
    messagePool!: QueryList<ElementRef>;
    @ViewChildren('messages')
    innerMessages!: QueryList<MessageObserverDirective>;
    @Output()
    messageSent = new EventEmitter<boolean>();
    isFirstUnread: boolean = true;
    firstUnreadId: number = 0;

    constructor(
        private authService: AuthorizeService,
        private client: Client,
        @Inject(LOCALE_ID) public locale: string
    ) {
        effect(() => {
            this.currentUserId = this.authService.getUserId()!;
        });
    }
    ngOnChanges(changes: SimpleChanges): void {
        if (
            changes['conversation'] !== null &&
            changes['conversation'] !== undefined
        ) {
            this.conversation = changes['conversation'].currentValue;
            this.getAllConversationMessages();
        }
    }

    ngAfterViewInit(): void {}

    async ngOnInit(): Promise<void> {
        this.getAllConversationMessages();
        this.updateChatEvent.subscribe({
            next: () => {
                this.getAllConversationMessages();
            },
        });
    }

    getAllConversationMessages() {
        this.client
            .getAllConversationChatMessages(this.conversation.id)
            .subscribe({
                next: (result) => {
                    result.chatMessages.map(
                        (el) => (el.timeStamp = new Date(el.timeStamp))
                    );

                    let mainWindowScrollHeightBefore =
                        this.mainConversationWindow.nativeElement.scrollHeight;
                    this.messages = this.groupMessagesByDate(
                        result.chatMessages
                    );

                    if (this.messagePool !== undefined) {
                        let subscriber = this.messagePool.changes.subscribe(
                            (i) => {
                                this.innerMessages.forEach((el) => {
                                    if (el.id == this.firstUnreadId) {
                                        console.log(
                                            el.id,
                                            this.firstUnreadId,
                                            el.offsetTop
                                        );
                                        console.log(
                                            this.mainConversationWindow
                                                .nativeElement.offsetHeight
                                        );
                                        this.mainConversationWindow.nativeElement.scrollTop =
                                            el.offsetTop -
                                            this.mainConversationWindow
                                                .nativeElement.offsetHeight;

                                        console.log(
                                            this.mainConversationWindow
                                                .nativeElement.scrollTop
                                        );
                                    }
                                });

                                subscriber.unsubscribe();
                            }
                        );
                    }
                },
            });
    }

    scrollBehaviour(before: any | null) {
        let mainWindowScrollHeight =
            before ?? this.mainConversationWindow.nativeElement.scrollHeight;
        let scrollTop =
            this.mainConversationWindow.nativeElement.scrollTop +
            this.mainConversationWindow.nativeElement.clientHeight;

        let lastDateGroup = this.messages.length - 1;
        let lastDateMessages = this.messages[lastDateGroup].messages.length - 1;
        let lastMessageSender =
            this.messages[lastDateGroup].messages[lastDateMessages].senderId;

        if (lastMessageSender == this.currentUserId) {
        } else {
            // if (scrollTop == mainWindowScrollHeight) {
            //     console.log('SCROLLTOP', scrollTop, mainWindowScrollHeight);
            //     this.scrollToBottom();
            // }
        }
    }

    scrollToBottom() {
        let mainWindowScrollHeight =
            this.mainConversationWindow.nativeElement.scrollHeight;
        this.mainConversationWindow.nativeElement.scrollTo({
            top: mainWindowScrollHeight,
            behavior: 'smooth',
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
                if (
                    this.isFirstUnread &&
                    !message.isRead &&
                    message.senderId != this.currentUserId
                ) {
                    this.firstUnreadId = message.id;
                    this.isFirstUnread = false;
                }

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

    sendMessage($event: Event, input: string) {
        $event.preventDefault();
        this.client.sendChatMessage(this.conversation.id, input).subscribe({
            next: () => {
                this.input.nativeElement.value = '';
                this.messageSent.emit(true);
                this.scrollToBottom();
                let subscriber = this.messagePool.changes.subscribe((i) => {
                    this.scrollToBottom();

                    subscriber.unsubscribe();
                });
            },
        });
    }

    generateMessageClass(message: ChatMessage) {
        if (this.currentUserId == message.senderId) {
            return 'self';
        } else return 'other';
    }

    intersect(event: IntersectResult) {
        if (event.isIntersecting && !event.isRead)
            this.client.chatMessageRead(event.messageId).subscribe({
                next: () => {
                    this.messageSent.emit(true);
                },
            });
    }
}
