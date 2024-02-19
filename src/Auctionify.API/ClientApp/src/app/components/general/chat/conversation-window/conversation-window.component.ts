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
    OnDestroy,
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
    isFirstLoadScrolled: boolean = false;
    firstUnreadId: number = 0;

    isScrolledUp: boolean = false;

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

    ngAfterViewInit(): void {
        this.mainConversationWindow.nativeElement.onscroll = (el) => {
            let percent = this.calculateCurrentScrollPercent();
            if (percent < 97.6) this.isScrolledUp = true;
            else this.isScrolledUp = false;
        };
    }

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

                    this.messages = this.groupMessagesByDate(
                        result.chatMessages
                    );

                    if (this.messagePool !== undefined) {
                        let subscriber = this.messagePool.changes.subscribe(
                            (i) => {
                                this.innerMessages.forEach((el) => {
                                    this.scrollBehaviour(el);
                                });

                                subscriber.unsubscribe();
                            }
                        );
                    }
                },
            });
    }

    scrollBehaviour(el: MessageObserverDirective | null) {
        if (!this.isScrolledUp) {
            if (el !== null && el.id == this.firstUnreadId) {
                this.mainConversationWindow.nativeElement.scrollTo({
                    top:
                        el.offsetTop -
                        this.mainConversationWindow.nativeElement.offsetHeight *
                            2 +
                        el.clientHeight,
                    behavior: 'smooth',
                });
            } else if (!this.isFirstLoadScrolled) {
                this.mainConversationWindow.nativeElement.scrollTo({
                    top: this.mainConversationWindow.nativeElement.scrollHeight,
                    behavior: 'smooth',
                });
                this.isFirstLoadScrolled = true;
            }
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
        if (input == null || input == undefined || input == '') return;
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
        if (event.isIntersecting && !event.isRead && !event.isOwner) {
            this.client.chatMessageRead(event.messageId).subscribe({
                next: () => {
                    this.messageSent.emit(true);
                },
            });
        }
    }

    calculateCurrentScrollPercent() {
        let scrollElement = this.mainConversationWindow.nativeElement;
        let scrollPos = scrollElement.scrollTop + scrollElement.clientHeight;

        return (scrollPos * 100) / scrollElement.scrollHeight;
    }
}
