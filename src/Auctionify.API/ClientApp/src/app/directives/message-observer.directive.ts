import {
    Directive,
    ElementRef,
    EventEmitter,
    Input,
    OnDestroy,
    OnInit,
    Output,
} from '@angular/core';
import { Observable, Subscription, debounceTime } from 'rxjs';

@Directive({
    selector: '[appMessageObserver]',
    exportAs: 'intersection',
})
export class MessageObserverDirective implements OnInit, OnDestroy {
    @Input() root: HTMLElement | null = null;
    @Input() rootMargin = '0px 0px 0px 0px';
    @Input() threshold = 0;
    @Input() debounceTime = 500;
    @Input() isContinuous = false;
    @Input() parentWindow: HTMLElement | null = null;
    @Input() isOwner: boolean = false;
    @Input() isRead: boolean = false;
    @Input() id: number = 0;
    public offsetTop = 0;
    public clientHeight = 0;

    @Output() isIntersecting = new EventEmitter<IntersectResult>();

    _isIntersecting = false;
    subscription!: Subscription;

    constructor(private element: ElementRef) {}

    ngOnInit() {
        this.subscription = this.createAndObserve();
        this.offsetTop = this.element.nativeElement.offsetTop;
        this.clientHeight = this.element.nativeElement.clientHeight;
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    createAndObserve() {
        const options: IntersectionObserverInit = {
            root: document,
            rootMargin: this.rootMargin,
            threshold: this.threshold,
        };

        return new Observable<boolean>((subscriber) => {
            const intersectionObserver = new IntersectionObserver((entries) => {
                const { isIntersecting } = entries[0];
                subscriber.next(isIntersecting);

                isIntersecting &&
                    !this.isContinuous &&
                    intersectionObserver.disconnect();
            }, options);

            if (!this.isOwner && !this.isRead)
                intersectionObserver.observe(this.element.nativeElement);

            return {
                unsubscribe() {
                    intersectionObserver.disconnect();
                },
            };
        })
            .pipe(debounceTime(this.debounceTime))
            .subscribe((status) => {
                const data: IntersectResult = {
                    isIntersecting: status,
                    messageId:
                        this.element.nativeElement.getAttribute('messageId'),
                    isRead: this.isRead,
                    element: this.element.nativeElement,
                };
                this.isIntersecting.emit(data);
                this._isIntersecting = status;
            });
    }
}

export interface IntersectResult {
    isIntersecting: boolean;
    messageId: number;
    isRead: boolean;
    element: HTMLElement;
}
