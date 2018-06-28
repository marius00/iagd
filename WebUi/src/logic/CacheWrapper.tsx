import * as NodeCache from 'node-cache';
import * as Guid from 'guid';

const cache = new NodeCache({stdTTL: 60 * 45});

function GetCached(key: string): string {
  let value = cache.get(key) as string;
  if (value === undefined) {
    value = Guid.raw();
    cache.set(key, value);
  }

  return value;
}

export default GetCached;