import { Component, Inject } from '@angular/core';
import { Headers, Http, RequestOptions } from '@angular/http';
import { Observable } from "rxjs/Observable";
import 'rxjs/add/observable/forkJoin';

@Component({
    selector: 'customers',
    templateUrl: './customers.component.html',
    styleUrls: ['./customers.component.css']
})
export class CustomersComponent {
    public customers: Customer[];
    public selectedCustomer: Customer | undefined;

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {
        this.refreshData();
    }

    async refreshData() {
        this.http.get(this.baseUrl + 'api/customers').subscribe(result => {
            let customerList = [];

            for (let stud of result.json() as Customer[]) {

                let customer = new Customer();
                customer.id = stud.id;
                customer.name = stud.name;
                customer.dateOfBirth = stud.dateOfBirth;
                customer.hasChanges = false;
                customerList.push(customer);
            }

            console.log("ok");

            this.customers = customerList;

            this.selectCustomer();
        }, error => console.error(error));
    }


    selectCustomer(): void {

        this.selectedCustomer = undefined;

        for (let stud of this.customers) {
            if (stud.deleted == false) {
                this.selectedCustomer = stud;
                break;
            }

        }
    }


    async putData(): Promise<void> {
        let headers = new Headers({ 'Content-Type': 'application/json' });

        let serverCalls = [];

        for (let customer of this.customers) {
            if (customer.hasChanges == true || customer.deleted) {

                let json = JSON.stringify(customer.toJSON());

                if (!customer.id) { //create
                    if (!customer.deleted) {
                        let call = this.http.put(this.baseUrl + 'api/customers', json, { headers: headers });
                        serverCalls.push(call);
                    }
                }
                else {
                    if (customer.deleted) {
                        let url = this.baseUrl + 'api/customers?id=' + customer.id;
                        let call = this.http.delete(url, { headers: headers });
                        serverCalls.push(call);
                    }
                    else {
                        let call = this.http.post(this.baseUrl + 'api/customers', json, { headers: headers });
                        serverCalls.push(call);
                    }

                }
            }
        }
        Observable.forkJoin(serverCalls)
            .subscribe(data => {
                this.refreshData();
            }, error => console.error(error));


    }

    onSelect(customer: Customer): void {

        if (customer.deleted == false) {
            this.selectedCustomer = customer;
        }
    }

    addNewCustomer(): void {
        this.selectedCustomer = new Customer();
        this.selectedCustomer.hasChanges = true;
        this.customers.push(this.selectedCustomer);
    }

    async saveChanges(): Promise<void> {
        await this.putData();
        //console.log("update completed");
        //await this.refreshData();
    }

    delete(customer: Customer): void {
        customer.deleted = true;
        this.selectCustomer();
    }
}

class Customer {
    id: number;

    private _name: string = "";
    private _dateOfBirth: Date;
    public hasChanges: boolean;
    public deleted: boolean = false;

    get name(): string {
        return this._name;
    }
    set name(n: string) {
        this._name = n;
        this.hasChanges = true;
        console.log("set name");
    }

    get dateOfBirth(): Date {
        return this._dateOfBirth;
    }
    set dateOfBirth(d: Date) {
        this._dateOfBirth = d;
        this.hasChanges = true;
        console.log("set dateOfBirth");
    }

    public toJSON() {
        return {
            id: this.id,
            name: this._name,
            dateOfBirth: this._dateOfBirth,
        };
    };
}
