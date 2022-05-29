export class FolderFile {
    public _id: string = "";
    constructor(
        public name: string,
        public dateCreated: Date,
        public lastUpdated: Date
    ) { }
}


export enum FolderType {
    Dataset,
    Model,
    Experiment
}