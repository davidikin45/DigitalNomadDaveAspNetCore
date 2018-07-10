import { AuthorAbstractBase } from "./author-abstract-base.model";
import { ShowForRead } from "../shows/shared/show-for-read.model";

export class AuthorForRead extends AuthorAbstractBase {
  id: number;
  shows: ShowForRead[];
}
