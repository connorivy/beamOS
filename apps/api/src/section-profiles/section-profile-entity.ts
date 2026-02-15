import {
  Area,
  AreaMomentOfInertia,
  Volume,
  WarpingMomentOfInertia,
} from "unitsnet-js";
import type { SectionProfileDomainEvent } from "./section-profile-events";
import { assertUuidV7 } from "../common/uuid";

export const SECTION_PROFILE_DISCRIMINATORS = [
  "STANDARD",
  "WITH_SHEAR_AREAS",
] as const;

export type SectionProfileDiscriminator =
  (typeof SECTION_PROFILE_DISCRIMINATORS)[number];

export type SectionProfileSnapshot = {
  id: string;
  revisionId: string;
  name: string;
  discriminator: SectionProfileDiscriminator;
  area: Area;
  strongAxisMomentOfInertia: AreaMomentOfInertia;
  weakAxisMomentOfInertia: AreaMomentOfInertia;
  torsionalConstant: AreaMomentOfInertia;
  warpingConstant: WarpingMomentOfInertia;
  strongAxisPlasticSectionModulus: Volume;
  weakAxisPlasticSectionModulus: Volume;
  strongAxisElasticSectionModulus: Volume;
  weakAxisElasticSectionModulus: Volume;
  strongAxisShearArea?: Area;
  weakAxisShearArea?: Area;
};

export class SectionProfileEntity {
  private _domainEvents: SectionProfileDomainEvent[];

  private constructor(snapshot: SectionProfileSnapshot) {
    assertUuidV7(snapshot.id, "id");
    assertUuidV7(snapshot.revisionId, "revisionId");

    if (snapshot.name.trim().length === 0) {
      throw new Error("name must be a non-empty string");
    }

    if (!SECTION_PROFILE_DISCRIMINATORS.includes(snapshot.discriminator)) {
      throw new Error(`Invalid discriminator \"${snapshot.discriminator}\"`);
    }

    this.assertArea(snapshot.area, "area");
    this.assertAreaMomentOfInertia(
      snapshot.strongAxisMomentOfInertia,
      "strongAxisMomentOfInertia",
    );
    this.assertAreaMomentOfInertia(
      snapshot.weakAxisMomentOfInertia,
      "weakAxisMomentOfInertia",
    );
    this.assertAreaMomentOfInertia(
      snapshot.torsionalConstant,
      "torsionalConstant",
    );
    this.assertWarpingMomentOfInertia(
      snapshot.warpingConstant,
      "warpingConstant",
    );
    this.assertVolume(
      snapshot.strongAxisPlasticSectionModulus,
      "strongAxisPlasticSectionModulus",
    );
    this.assertVolume(
      snapshot.weakAxisPlasticSectionModulus,
      "weakAxisPlasticSectionModulus",
    );
    this.assertVolume(
      snapshot.strongAxisElasticSectionModulus,
      "strongAxisElasticSectionModulus",
    );
    this.assertVolume(
      snapshot.weakAxisElasticSectionModulus,
      "weakAxisElasticSectionModulus",
    );

    if (snapshot.discriminator === "WITH_SHEAR_AREAS") {
      if (!snapshot.strongAxisShearArea || !snapshot.weakAxisShearArea) {
        throw new Error(
          "strongAxisShearArea and weakAxisShearArea are required for WITH_SHEAR_AREAS",
        );
      }
    }

    if (
      snapshot.discriminator === "STANDARD" &&
      (snapshot.strongAxisShearArea || snapshot.weakAxisShearArea)
    ) {
      throw new Error(
        "strongAxisShearArea and weakAxisShearArea are only allowed for WITH_SHEAR_AREAS",
      );
    }

    if (snapshot.strongAxisShearArea) {
      this.assertArea(snapshot.strongAxisShearArea, "strongAxisShearArea");
    }

    if (snapshot.weakAxisShearArea) {
      this.assertArea(snapshot.weakAxisShearArea, "weakAxisShearArea");
    }

    this.id = snapshot.id;
    this.revisionId = snapshot.revisionId;
    this.name = snapshot.name;
    this.discriminator = snapshot.discriminator;
    this.area = snapshot.area;
    this.strongAxisMomentOfInertia = snapshot.strongAxisMomentOfInertia;
    this.weakAxisMomentOfInertia = snapshot.weakAxisMomentOfInertia;
    this.torsionalConstant = snapshot.torsionalConstant;
    this.warpingConstant = snapshot.warpingConstant;
    this.strongAxisPlasticSectionModulus = snapshot.strongAxisPlasticSectionModulus;
    this.weakAxisPlasticSectionModulus = snapshot.weakAxisPlasticSectionModulus;
    this.strongAxisElasticSectionModulus = snapshot.strongAxisElasticSectionModulus;
    this.weakAxisElasticSectionModulus = snapshot.weakAxisElasticSectionModulus;
    this.strongAxisShearArea = snapshot.strongAxisShearArea;
    this.weakAxisShearArea = snapshot.weakAxisShearArea;
    this._domainEvents = [];
  }

  readonly id: string;
  readonly revisionId: string;
  readonly name: string;
  readonly discriminator: SectionProfileDiscriminator;
  readonly area: Area;
  readonly strongAxisMomentOfInertia: AreaMomentOfInertia;
  readonly weakAxisMomentOfInertia: AreaMomentOfInertia;
  readonly torsionalConstant: AreaMomentOfInertia;
  readonly warpingConstant: WarpingMomentOfInertia;
  readonly strongAxisPlasticSectionModulus: Volume;
  readonly weakAxisPlasticSectionModulus: Volume;
  readonly strongAxisElasticSectionModulus: Volume;
  readonly weakAxisElasticSectionModulus: Volume;
  readonly strongAxisShearArea?: Area;
  readonly weakAxisShearArea?: Area;

  static create(snapshot: SectionProfileSnapshot): SectionProfileEntity {
    const entity = new SectionProfileEntity(snapshot);
    entity._domainEvents.push({
      type: "section_profile_created",
      payload: entity.toSnapshot(),
    });
    return entity;
  }

  static rehydrate(snapshot: SectionProfileSnapshot): SectionProfileEntity {
    return new SectionProfileEntity(snapshot);
  }

  toSnapshot(): SectionProfileSnapshot {

    return {
      id: this.id,
      revisionId: this.revisionId,
      name: this.name,
      discriminator: this.discriminator,
      area: this.area,
      strongAxisMomentOfInertia: this.strongAxisMomentOfInertia,
      weakAxisMomentOfInertia: this.weakAxisMomentOfInertia,
      torsionalConstant: this.torsionalConstant,
      warpingConstant: this.warpingConstant,
      strongAxisPlasticSectionModulus: this.strongAxisPlasticSectionModulus,
      weakAxisPlasticSectionModulus: this.weakAxisPlasticSectionModulus,
      strongAxisElasticSectionModulus: this.strongAxisElasticSectionModulus,
      weakAxisElasticSectionModulus: this.weakAxisElasticSectionModulus,
      strongAxisShearArea: this.strongAxisShearArea,
      weakAxisShearArea: this.weakAxisShearArea,
    };
  }

  pullDomainEvents(): SectionProfileDomainEvent[] {
    const events = [...this._domainEvents];
    this._domainEvents = [];
    return events;
  }

  private assertArea(value: Area, field: string): void {
    if (!(value instanceof Area) || !Number.isFinite(value.BaseValue)) {
      throw new Error(`${field} must be a finite Area`);
    }
  }

  private assertAreaMomentOfInertia(
    value: AreaMomentOfInertia,
    field: string,
  ): void {
    if (
      !(value instanceof AreaMomentOfInertia) ||
      !Number.isFinite(value.BaseValue)
    ) {
      throw new Error(`${field} must be a finite AreaMomentOfInertia`);
    }
  }

  private assertWarpingMomentOfInertia(
    value: WarpingMomentOfInertia,
    field: string,
  ): void {
    if (
      !(value instanceof WarpingMomentOfInertia) ||
      !Number.isFinite(value.BaseValue)
    ) {
      throw new Error(`${field} must be a finite WarpingMomentOfInertia`);
    }
  }

  private assertVolume(value: Volume, field: string): void {
    if (!(value instanceof Volume) || !Number.isFinite(value.BaseValue)) {
      throw new Error(`${field} must be a finite Volume`);
    }
  }
}
