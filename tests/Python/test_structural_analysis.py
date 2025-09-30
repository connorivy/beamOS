import beamos_structuralanalysis as bsa
import uuid

def test_model():
    builder = bsa.sdk.BeamOsDynamicModel(
        uuid.UUID("bb993099-3b48-4311-bddb-681f015da825"))

    builder.add_node(1, 0, 0, 0)
    builder.add_node(2, 1, 0, 0)
    builder.add_node(3, 1, 1, 0)
    builder.add_node(4, 0, 1, 0)

    builder.add_material(1, "Steel", 210e9, 7850)
    builder.add_section_profile_from_library(1, "w12x26", bsa.contracts.physicalmodel.sectionprofiles.StructuralCode.aisc_360_16)

    builder.add_element_1d(1, 1, 2, 1, 1)
    builder.add_element_1d(2, 2, 3, 1, 1)
    builder.add_element_1d(3, 3, 4, 1, 1)
    builder.add_element_1d(4, 4, 1, 1, 1)
