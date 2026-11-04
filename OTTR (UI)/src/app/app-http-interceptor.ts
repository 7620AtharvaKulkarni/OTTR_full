import { Injectable } from "@angular/core";
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Observable } from 'rxjs';

@Injectable()
export class AppHttpInterceptor implements HttpInterceptor {
    constructor() { }
    private headers: string[][] = [
        ["Cache-Control", "no-store"],
        ["Pragma", "no-cache"],
        ["X-Content-Type-Options", "nosniff"]
    ];
    intercept(req: HttpRequest<any>, next: HttpHandler):
        Observable<HttpEvent<any>> {
        for (var i = 0; i < this.headers.length; i++) {
            if (req.headers.has(this.headers[i][0])) {
                req = req.clone({
                    headers: req.headers.delete(this.headers[i]
                    [0])
                });
            }
            req = req.clone({
                headers: req.headers.set(this.headers[i][0],
                    this.headers[i][1])
            });
        }
        return next.handle(req);
    }
}